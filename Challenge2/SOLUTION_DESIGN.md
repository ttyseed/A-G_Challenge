# Challenge #2: BFF Design for Multi-Services Platform

## Table of Contents
1. [Overview & Assumptions](#1-overview--assumptions)
2. [Architecture Overview](#2-architecture-overview)
3. [Architecture Diagram](#3-architecture-diagram)
4. [BFF Design](#4-bff-design)
5. [Assumed Microservice Endpoints](#5-assumed-microservice-endpoints)
6. [Technology Stack](#6-technology-stack)
7. [Deployment Strategy](#7-deployment-strategy)
8. [Data Consistency & Integrity](#8-data-consistency--integrity)
9. [Security & PII Compliance](#9-security--pii-compliance)
10. [Observability Strategy](#10-observability-strategy)
11. [Resiliency Patterns](#11-resiliency-patterns)
12. [Trade-offs & Recommendations](#12-trade-offs--recommendations)

---

## 1. Overview & Assumptions

### Context
An insurance platform (A&G SEA) exposes two portals:
- **Customer Portal** — end-users purchase insurance policies directly (self-service)
- **Agent Portal** — licensed agents purchase policies on behalf of customers

Two backend microservices already exist:
- **Quotation Service** — manages quote lifecycle (creation, comparison, binding)
- **Payment Service** — manages payment lifecycle (initiation, confirmation, refund)

### Assumptions

| Area | Assumption |
|---|---|
| Users | Customers authenticate via email/social login; Agents use enterprise SSO |
| Quotes | A quote must be created before a policy can be bound |
| Payment | Payment is required to bind a quote into a policy |
| PII | Customer name, NRIC/FIN, DOB, address, contact info are classified PII |
| Compliance | MAS TRM (Singapore) guidelines apply; PCI-DSS for card payments; AWS Singapore region (ap-southeast-1) |
| Scale | ~10,000 concurrent users; ~500 agent users |
| Infra | Cloud-native on AWS (EKS + managed AWS services) |
| Payment Gateway | Third-party gateway (Stripe or PayNow/NETS for SGD) |

---

## 2. Architecture Overview

The BFF (Backend-for-Frontend) pattern is applied to create **two distinct BFF services** — one per portal. Each BFF acts as an orchestration and adaptation layer, aggregating calls to downstream microservices and shaping responses to the exact needs of its frontend client.

### Why Two BFFs Instead of One?

| Concern | Single BFF | Dual BFF |
|---|---|---|
| API shape | Compromise between portals | Optimised per client |
| Auth model | Mixed (customer + agent) | Isolated per identity provider |
| Release cadence | Coupled | Independent |
| Security surface | Larger | Smaller, scoped |
| Team ownership | Shared | Clear boundary |

### Key Design Principles
- **Separation of concerns**: BFF owns orchestration; microservices own domain logic
- **Zero trust**: All service-to-service calls are authenticated via mTLS + JWT
- **Least privilege**: BFF tokens carry only scopes needed per portal
- **No PII in logs**: PII is masked/redacted at the BFF layer before any log emission
- **Idempotency**: All payment-related mutations use idempotency keys

---

## 3. Architecture Diagram

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                              EXTERNAL CLIENTS                                │
│                                                                              │
│   ┌─────────────────────┐              ┌─────────────────────┐              │
│   │   Customer Portal   │              │    Agent Portal     │              │
│   │  (React / Next.js)  │              │  (React / Next.js)  │              │
│   └────────┬────────────┘              └──────────┬──────────┘              │
└────────────┼──────────────────────────────────────┼─────────────────────────┘
             │ HTTPS / TLS 1.3                       │ HTTPS / TLS 1.3
             ▼                                       ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│                   AWS CloudFront + AWS WAF                                   │
│  • DDoS protection (AWS Shield Standard)   • OWASP managed rule groups       │
│  • TLS termination at edge                 • Geo-restriction (optional)      │
│  • IP allowlisting for Agent portal        • Rate limiting per IP            │
└──────────┬──────────────────────────────────────┬───────────────────────────┘
           │                                      │
           ▼                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        Amazon API Gateway (HTTP API)                        │
│  • Route /customer/* → Customer BFF          • JWT authoriser               │
│  • Route /agent/*    → Agent BFF             • Throttling & usage plans     │
└──────────┬──────────────────────────────────────┬──────────────────────────┘
           │                                      │
           ▼                                      ▼
┌──────────────────────┐              ┌───────────────────────┐
│   Customer BFF       │              │     Agent BFF         │
│  (ASP.NET Core 8)    │              │  (ASP.NET Core 8)     │
│  Deployed on EKS     │              │  Deployed on EKS      │
│                      │              │                       │
│  Auth: Amazon        │              │  Auth: AWS IAM        │
│  Cognito User Pool   │              │  Identity Center      │
│  (OAuth2/OIDC)       │              │  (SAML/OIDC + MFA)    │
│                      │              │                       │
│  Aggregates:         │              │  Aggregates:          │
│  • Quotation API     │              │  • Quotation API      │
│  • Payment API       │              │  • Payment API        │
│  • User Profile      │              │  • Customer Mgmt      │
│                      │              │  • Commission Calc    │
│  ElastiCache Redis   │              │  ElastiCache Redis    │
└──────────┬───────────┘              └──────────┬────────────┘
           │                                      │
           └──────────────┬───────────────────────┘
                          │  Private VPC (mTLS via AWS App Mesh)
                          ▼
         ┌────────────────────────────────────┐
         │          AWS APP MESH              │
         │    (Envoy sidecar on EKS)          │
         │  • mTLS between all services       │
         │  • Circuit breaker                 │
         │  • Retry / timeout policies        │
         └──────┬─────────────────┬───────────┘
                │                 │
                ▼                 ▼
  ┌─────────────────────┐   ┌──────────────────────┐
  │  Quotation Service  │   │   Payment Service    │
  │  (ASP.NET Core 8)   │   │   (ASP.NET Core 8)   │
  │  on EKS             │   │   on EKS             │
  │                     │   │                      │
  │  POST /quotes       │   │  POST /payments      │
  │  GET  /quotes/{id}  │   │  GET  /payments/{id} │
  │  PUT  /quotes/{id}  │   │  POST /payments/     │
  │  POST /quotes/bind  │   │       {id}/confirm   │
  │  GET  /quotes/      │   │  POST /payments/     │
  │       compare       │   │       {id}/refund    │
  │                     │   │  POST /webhooks/     │
  │  DB: Amazon RDS     │   │       gateway        │
  │  (PostgreSQL)       │   │                      │
  └─────────────────────┘   │  DB: Amazon RDS      │
                             │  (PostgreSQL)        │
                             └──────────┬───────────┘
                                        │
                                        ▼
                             ┌──────────────────────┐
                             │   PAYMENT GATEWAY    │
                             │ (Stripe / PayNow)    │
                             │                      │
                             │  • Card processing   │
                             │  • PayNow QR (SGD)   │
                             │  • Webhook events    │
                             └──────────────────────┘

  ┌──────────────────────────────────────────────────────────────────────────┐
  │                      SHARED AWS INFRASTRUCTURE                           │
  │                                                                          │
  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────────────┐  │
  │  │  AWS Secrets │  │  Amazon      │  │  Amazon SQS + SNS /          │  │
  │  │  Manager +   │  │  ElastiCache │  │  Amazon EventBridge          │  │
  │  │  AWS KMS     │  │  for Redis   │  │  (Async events:              │  │
  │  │  (Secrets,   │  │  (Sessions,  │  │   payment.confirmed          │  │
  │  │   CMKs,      │  │   Quote      │  │   quote.bound                │  │
  │  │   Rotation)  │  │   drafts)    │  │   policy.issued)             │  │
  │  └──────────────┘  └──────────────┘  └──────────────────────────────┘  │
  │                                                                          │
  │  ┌────────────────────────────────────────────────────────────────┐     │
  │  │                   OBSERVABILITY STACK                          │     │
  │  │  AWS CloudWatch Logs + Metrics | AWS X-Ray (distributed traces)│     │
  │  │  Amazon Managed Grafana | Amazon Managed Service for Prometheus│     │
  │  └────────────────────────────────────────────────────────────────┘     │
  │                                                                          │
  │  ┌────────────────────────────────────────────────────────────────┐     │
  │  │                   SECURITY STACK                               │     │
  │  │  AWS Security Hub | Amazon GuardDuty | AWS Config              │     │
  │  │  Amazon Macie (PII detection in S3) | AWS CloudTrail           │     │
  │  └────────────────────────────────────────────────────────────────┘     │
  └──────────────────────────────────────────────────────────────────────────┘
```

---

## 4. BFF Design

### 4.1 Customer BFF

**Responsibility**: Serve the self-service customer journey for purchasing an insurance policy.

**Key Flows:**

```
1. Get Quote
   Client → Customer BFF → Quotation Service
   BFF aggregates product catalogue + quote result into single response

2. Compare Quotes
   Client → Customer BFF → Quotation Service (/compare)
   BFF formats comparison view suitable for consumer UI

3. Initiate Payment
   Client → Customer BFF → Payment Service (POST /payments)
   BFF attaches idempotency key; card tokenisation handled by Stripe.js in browser

4. Poll Payment Status
   Client → Customer BFF → Payment Service (GET /payments/{id})
   BFF translates payment state machine into UI-friendly status

5. Policy Confirmation
   Payment Gateway → Payment Service webhook → SQS (payment.confirmed)
   Payment Service → EventBridge → Quotation Service (bind)
   Customer BFF → WebSocket/SSE push to client (via API Gateway WebSocket API)
```

**Tailored features:**
- Simplified quote form (fewer fields; pre-filled where possible)
- Payment via card or PayNow QR
- Policy document download via pre-signed S3 URL (TTL 5 min)
- Session-based quote draft (ElastiCache Redis TTL: 30 min)

### 4.2 Agent BFF

**Responsibility**: Serve licensed agents managing policy purchases for multiple customers.

**Key Flows:**

```
1. Customer Lookup / Creation
   Agent → Agent BFF → Customer Service (internal)
   BFF enforces data minimisation — only fields needed for quoting

2. Quote on Behalf of Customer
   Agent → Agent BFF → Quotation Service
   BFF injects agent_id + customer_id into downstream requests for full auditability

3. Override / Apply Discounts
   Agent → Agent BFF → Quotation Service (PATCH /quotes/{id}/adjustments)
   BFF validates agent has discount_authority scope (from Cognito/IAM Identity Center token claims)

4. Payment Collection
   Agent → Agent BFF → Payment Service
   Supports additional payment methods (cheque reference, bank transfer for corporates)

5. Commission Tracking
   Agent BFF → Commission Service (internal) — aggregated alongside quote summary
```

**Tailored features:**
- Bulk quote management (pagination, filtering, export to S3-backed CSV)
- Customer PII viewable only after re-authentication step-up (Cognito advanced security)
- Audit trail per action logged to CloudTrail + immutable S3 (Object Lock)
- Role-based access: `agent`, `senior_agent`, `supervisor` with tiered permissions via Cognito Groups

---

## 5. Assumed Microservice Endpoints

### Quotation Service

| Method | Path | Description |
|---|---|---|
| POST | /quotes | Create a new quote |
| GET | /quotes/{id} | Retrieve quote by ID |
| PUT | /quotes/{id} | Update quote details |
| DELETE | /quotes/{id} | Cancel / discard quote |
| POST | /quotes/{id}/bind | Bind quote into a policy (requires confirmed payment) |
| GET | /quotes/compare | Compare multiple quote options |
| GET | /quotes?customerId={id} | List quotes for a customer |
| PATCH | /quotes/{id}/adjustments | Apply agent discount/override |

### Payment Service

| Method | Path | Description |
|---|---|---|
| POST | /payments | Initiate a payment session |
| GET | /payments/{id} | Get payment status |
| POST | /payments/{id}/confirm | Confirm a pending payment (3DS callback) |
| POST | /payments/{id}/refund | Initiate refund |
| GET | /payments?customerId={id} | Payment history for customer |
| POST | /webhooks/gateway | Receive async events from payment gateway |

---

## 6. Technology Stack

| Layer | AWS / Technology | Rationale |
|---|---|---|
| BFF runtime | ASP.NET Core 8 (minimal API) on Amazon EKS | Kubernetes-native; consistent .NET stack |
| Edge / CDN | Amazon CloudFront + AWS WAF | Global edge caching; OWASP managed rules; Shield Standard DDoS |
| API Gateway | Amazon API Gateway (HTTP API) | JWT authoriser, throttling, WebSocket support for push notifications |
| Auth (Customer) | Amazon Cognito User Pools | Managed IdP; social federation (Google/Apple); built-in MFA; hosted UI |
| Auth (Agent) | AWS IAM Identity Center | Enterprise SSO via SAML 2.0 / OIDC; MFA enforcement; AD integration |
| Service Mesh | AWS App Mesh (Envoy) on EKS | mTLS, traffic shaping, circuit breaking; native AWS integration |
| Cache | Amazon ElastiCache for Redis (cluster mode) | Quote draft persistence, session tokens, idempotency store |
| Messaging | Amazon SQS (standard + FIFO) + Amazon EventBridge | SQS for reliable delivery; EventBridge for event routing between services |
| Secrets | AWS Secrets Manager + AWS KMS (CMK) | Automatic rotation; KMS for envelope encryption of PII columns |
| Databases | Amazon RDS PostgreSQL (Multi-AZ) | ACID guarantees; Multi-AZ standby for HA; read replicas for reporting |
| Object Storage | Amazon S3 (S3 Object Lock for audit logs) | Policy documents, CSV exports, immutable audit trail |
| Container Registry | Amazon ECR | Integrated with EKS; image scanning via ECR Enhanced Scanning |
| CI/CD | GitHub Actions + AWS CodePipeline / CodeBuild | SAST (Semgrep), DAST (OWASP ZAP), image scan gates in pipeline |
| IaC | AWS CDK (TypeScript) or Terraform | CDK for AWS-native idioms; Terraform for multi-cloud portability |
| Feature Flags | AWS AppConfig | Gradual rollout; instant kill-switch without redeployment |
| Security Posture | AWS Security Hub + Amazon GuardDuty + Amazon Macie | Threat detection; PII discovery in S3; compliance score |
| Observability | AWS CloudWatch + X-Ray + Amazon Managed Grafana + AMP | Full telemetry stack (logs, traces, metrics) |

---

## 7. Deployment Strategy

### AWS Account Structure (AWS Organizations)

```
Root
├── Security Account      (GuardDuty delegated admin, Security Hub aggregator)
├── Shared Services       (ECR, shared Secrets Manager, Route 53 hosted zones)
├── Dev Account           (EKS dev cluster, RDS dev, Cognito dev user pool)
├── Staging Account       (mirrors production topology)
└── Production Account    (EKS prod cluster, RDS Multi-AZ, WAF, CloudFront)
```

### EKS Cluster Layout

```
Production EKS (ap-southeast-1)
├── Namespace: customer-bff
│   ├── Deployment (min 2 replicas, HPA max 10, CPU target 60%)
│   ├── Service (ClusterIP)
│   └── App Mesh VirtualService
├── Namespace: agent-bff
│   ├── Deployment (min 2 replicas, HPA max 5)
│   └── App Mesh VirtualService
├── Namespace: quotation-svc
├── Namespace: payment-svc
└── Namespace: monitoring
```

### Multi-AZ & High Availability

- EKS nodes spread across 3 AZs (ap-southeast-1a/b/c) via topology spread constraints
- RDS PostgreSQL deployed Multi-AZ with automatic failover (< 60s RTO)
- ElastiCache Redis in cluster mode with 2 replicas per shard
- SQS and EventBridge are regionally durable by default

### CI/CD Pipeline (GitHub Actions + CodePipeline)

```
Push to main branch
  └── GitHub Actions
        ├── Unit tests
        ├── SAST scan (Semgrep / CodeQL)
        ├── Docker build → push to ECR
        ├── ECR Enhanced Scan (gate: no CRITICAL CVEs)
        └── Trigger CodePipeline

CodePipeline
  ├── Deploy to dev (auto)
  ├── Integration tests + DAST (OWASP ZAP)
  ├── Deploy to staging (auto)
  ├── Smoke tests + synthetic canary
  └── Deploy to production (manual approval gate)
        └── Canary deployment (10% → 50% → 100% over 15 min)
              └── CloudWatch alarm rollback on error spike
```

### Zero-Downtime Deployment
- **Rolling updates** with `maxSurge: 1, maxUnavailable: 0`
- **Readiness probes** on `/health/ready` before traffic shifts
- **AWS AppConfig** feature flags for gradual feature rollout independent of deployments

---

## 8. Data Consistency & Integrity

### The Core Problem: Quote Binding + Payment

The most critical consistency concern is the two-step flow: **payment must succeed before a quote is bound as a policy**. These span two microservices — a distributed transaction problem.

### Solution: Saga Pattern (Choreography via EventBridge + SQS)

```
1. Customer BFF calls Payment Service: POST /payments
   → Payment Service records payment in PENDING state (RDS)
   → Payment Service calls Stripe; on success publishes to EventBridge
     Event: { source: "payment-service", detail-type: "payment.confirmed", ... }

2. EventBridge rule routes payment.confirmed → SQS queue (Quotation Service subscriber)
   → Quotation Service consumes from SQS (visibility timeout: 30s, DLQ after 3 retries)
   → Validates quote is still in ACCEPTED state
   → Transitions quote to BOUND, creates Policy record
   → Publishes quote.bound event to EventBridge

3. Customer BFF subscribes to quote.bound via API Gateway WebSocket
   → Notifies client (policy issued confirmation + pre-signed S3 URL for PDF)

Compensation (failure path):
   Gateway failure → payment.failed event → quote remains ACCEPTED (customer retries)
   Bind failure after payment.confirmed → Payment Service auto-refund via SQS dead-letter alarm
   DLQ depth > 0 → CloudWatch alarm → PagerDuty + Lambda handler for triage
```

### Idempotency

All payment-initiating requests include an `Idempotency-Key` header (UUID v4 generated client-side).  
Payment Service stores this key in ElastiCache Redis with TTL=24h and returns the cached response on duplicates — prevents double-charges on network retries.

### Data Integrity Controls

| Layer | Control |
|---|---|
| RDS | Multi-AZ with automated backups; point-in-time recovery (35-day retention) |
| Schema | Optimistic concurrency (row versioning); foreign key constraints |
| API | Input validation (FluentValidation); request schema versioning |
| Events | EventBridge Schema Registry; consumers validate schema before processing |
| Audit | Append-only audit table + S3 Object Lock (WORM) for immutable audit trail |
| Encryption | RDS encrypted with KMS CMK; S3 SSE-KMS; ElastiCache in-transit + at-rest |

---

## 9. Security & PII Compliance

### Authentication & Authorization

```
Customer Portal:
  OAuth2 Authorization Code Flow + PKCE via Amazon Cognito User Pool
  Tokens: Access token (15 min TTL) + Refresh token (24h, rotating)
  MFA: Optional for customers; mandatory for transactions > SGD 5,000
  Social federation: Google, Apple via Cognito Identity Federation

Agent Portal:
  OAuth2 via AWS IAM Identity Center (OIDC) — backed by corporate Active Directory
  MFA: Mandatory for all agents (IAM Identity Center MFA policy)
  Step-up auth: Cognito Advanced Security triggers re-authentication to expose PII
  Session: 8-hour max with forced re-login; idle timeout 30 min
```

### Authorization Model (RBAC + ABAC)

```
Cognito Groups map to application roles:
  customer         → own quotes/payments only
  agent            → assigned customer quotes; no discount authority
  senior_agent     → discount_authority up to 15%
  supervisor       → full read on agent portfolio; approve discounts > 15%

ABAC via token claims:
  agent:customer_ids  → agent can only access their assigned customers
  quote:owner_agent   → quote only modifiable by creating agent or supervisor
  BFF enforces claims before forwarding to microservices
```

### PII Compliance (MAS TRM + PDPA)

| Requirement | AWS Implementation |
|---|---|
| Data minimisation | BFF strips excess fields from microservice responses before sending to client |
| PII masking in logs | OpenTelemetry processor redacts NRIC, DOB, phone, email; CloudWatch Log Data Protection auto-masks PII patterns |
| Encryption at rest | RDS encrypted with KMS CMK; sensitive columns (NRIC, DOB) use application-level envelope encryption |
| Encryption in transit | TLS 1.3 everywhere; App Mesh mTLS within cluster (ACM Private CA) |
| PII access logging | All PII field access logged to CloudTrail + S3 Object Lock (immutable, 7-year retention) |
| PII discovery | Amazon Macie scans S3 buckets and alerts on unexpected PII exposure |
| Right to erasure | Soft delete — PII replaced with pseudonymous token; original encrypted value in Secrets Manager |
| Data residency | All resources in `ap-southeast-1` (Singapore); S3 bucket policy denies cross-region replication |

### Payment Security (PCI-DSS)

- **No card data touches BFF or microservices** — Stripe.js tokenises in the browser
- BFF receives only a `paymentMethodToken` — never raw card numbers
- Payment Service stores only gateway transaction IDs and masked PAN (`**** **** **** 4242`)
- Payment Service pods in a dedicated EKS namespace + VPC subnet with restrictive Security Group (deny all inbound except App Mesh)
- PCI-DSS scope reduced to the Stripe integration boundary only
- AWS Security Hub with PCI-DSS standard enabled for continuous compliance checks

### Secrets Management

```
No secrets in code, Dockerfiles, or Kubernetes Secrets (base64 ≠ encrypted).
All secrets stored in AWS Secrets Manager; fetched at pod startup via:
  - AWS Secrets & Configuration Provider (ASCP) for Kubernetes
  - IAM Role for Service Account (IRSA) — pods assume scoped IAM roles
Automatic rotation: RDS passwords every 30 days; API keys every 90 days.
KMS CMK with key rotation enabled (annual).
```

### Network Security

```
VPC design:
  Public subnets:    CloudFront origin (ALB only)
  Private subnets:   EKS nodes, RDS, ElastiCache
  Isolated subnets:  Payment Service (stricter NSG)

Security Groups:
  ALB → BFF pods: 443 only
  BFF → Quotation/Payment: App Mesh port only
  Payment → RDS: 5432 only, source = payment SG only
  No direct internet access from any pod (NAT Gateway for egress)

AWS Network Firewall on egress:
  Allowlist: Stripe API domains, PayNow endpoints only
  All other outbound blocked
```

---

## 10. Observability Strategy

### Three Pillars

#### Logs
- **Structured JSON logging** (Serilog) exported via Fluent Bit DaemonSet → CloudWatch Logs
- Correlation ID (`X-Correlation-ID` header) propagated end-to-end and included in every log line
- **CloudWatch Log Data Protection** automatically masks PII patterns (NRIC regex, card numbers) before indexing
- Log Groups per service with retention: 90 days hot, archived to S3 Glacier after 90 days (1-year total)
- CloudWatch Logs Insights for ad-hoc queries; metric filters for alerting

#### Metrics
- **RED metrics** (Rate, Errors, Duration) per BFF endpoint via CloudWatch custom metrics
- EKS metrics via Container Insights (CloudWatch Agent + Fluent Bit)
- Application metrics exported via OpenTelemetry → **Amazon Managed Service for Prometheus (AMP)**
- Custom business metrics: quote conversion rate, payment success rate, quote-to-bind latency
- Visualised on **Amazon Managed Grafana** dashboards (pre-built + custom)

SLO targets:
- p99 latency < 500ms
- Payment success rate > 99.5%
- Error rate < 0.1%

#### Traces
- **AWS X-Ray** for distributed tracing (X-Ray SDK in ASP.NET Core + Envoy X-Ray plugin)
- Trace spans cover: CloudFront → API Gateway → BFF → App Mesh → Microservice → RDS
- Sampling: 5% for healthy traffic; 100% for error traces (X-Ray sampling rules)
- X-Ray Service Map provides visual topology of all service dependencies
- OpenTelemetry traces also exported to X-Ray via ADOT Collector (AWS Distro for OpenTelemetry)

### Alerting Strategy

| Signal | Threshold | Action |
|---|---|---|
| Payment error rate | > 1% over 5 min | PagerDuty P1 — on-call engineer |
| Customer BFF p99 latency | > 1s sustained 10 min | PagerDuty P2 — team Slack alert |
| Quote binding failures | > 5 consecutive | PagerDuty P1 — immediate |
| SQS DLQ depth | > 0 messages | CloudWatch alarm → Lambda → Slack alert |
| Secrets Manager access failure | Any | PagerDuty P1 — security team |
| GuardDuty finding (HIGH) | Any | SNS → Security Hub → Slack + email |
| Macie PII finding in S3 | Any | CloudWatch Events → Lambda → Security ticket |
| RDS failover event | Any | SNS → Slack notification |

### Dashboards (Amazon Managed Grafana)

```
1. Platform Health Dashboard
   - BFF request volume, error rate, latency per portal (Customer vs Agent)
   - Downstream service health (Quotation, Payment) via X-Ray Service Map
   - Payment gateway response times

2. Business Metrics Dashboard
   - Quote funnel: created → compared → bound (conversion rates)
   - Payment success / failure / refund rates
   - Agent productivity (quotes raised per agent per day)

3. Security & Compliance Dashboard
   - GuardDuty findings by severity over time
   - WAF blocked requests by rule
   - Failed authentication attempts (Cognito / IAM Identity Center)
   - PII access volume vs baseline (Macie)

4. SLO Dashboard
   - Error budget burn rate (Grafana SLO plugin)
   - Latency percentiles (p50/p95/p99) per service
```

### Synthetic Monitoring
- **CloudWatch Synthetics** canaries simulate end-to-end flows every 5 minutes:
  - Customer canary: create quote → initiate payment → confirm → check policy status
  - Agent canary: look up customer → create quote → apply discount → bind
- Run from multiple AWS regions (ap-southeast-1, ap-east-1) to detect regional issues
- Canary failures trigger CloudWatch alarms → PagerDuty

---

## 11. Resiliency Patterns

### Circuit Breaker (Polly + App Mesh)

```csharp
// BFF outbound HTTP client — Quotation Service
services.AddHttpClient<IQuotationClient, QuotationClient>()
    .AddPolicyHandler(Policy
        .Handle<HttpRequestException>()
        .CircuitBreakerAsync(
            exceptionsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)))
    .AddPolicyHandler(Policy
        .Handle<HttpRequestException>()
        .WaitAndRetryAsync(3, retry => TimeSpan.FromMilliseconds(200 * retry)));
```

App Mesh outlier detection configured at the VirtualNode level provides a complementary circuit breaker at the infrastructure layer, independent of the application.

### Fallback Strategies

| Scenario | Fallback |
|---|---|
| Quotation Service unavailable | Return cached quote (ElastiCache Redis, TTL 5 min) with `X-Cache-Stale: true` header |
| Payment Service unavailable | Queue payment intent to SQS FIFO; process when service recovers; client shown "payment pending" |
| Payment Gateway timeout | Return `payment_pending` status; gateway webhook async confirms result |
| ElastiCache unavailable | Degrade gracefully — bypass cache, hit microservice directly |
| RDS primary failure | Multi-AZ automatic failover (< 60s); connection pool reconnects automatically |

### Timeout Budgets

| Call | Timeout |
|---|---|
| Client → API Gateway → BFF | 29s (API Gateway hard limit) |
| BFF → Quotation Service | 3s |
| BFF → Payment Service | 5s |
| Payment Service → Gateway | 10s |
| Overall BFF response budget | 8s (with graceful degradation) |

### Health Checks

Each BFF exposes:
- `GET /health/live` — liveness: is the process running?
- `GET /health/ready` — readiness: can it accept traffic? (checks ElastiCache + Secrets Manager + downstream)
- `GET /health/startup` — startup: has it initialised? (secrets loaded, DB warm)

EKS liveness/readiness probes configured on these endpoints; failed probes trigger pod restart or traffic removal.

### AWS Fault Injection Simulator (FIS)
Scheduled chaos experiments validate resiliency:
- Kill 1 BFF pod → verify HPA spins up replacement; no dropped requests
- Inject 50% latency into Quotation Service → verify circuit breaker trips and stale cache serves
- Simulate AZ failure → verify Multi-AZ RDS failover and pod rescheduling

---

## 12. Trade-offs & Recommendations

### Trade-offs

| Decision | Alternative Considered | Why Chosen |
|---|---|---|
| Two BFFs | GraphQL Federation (AWS AppSync) | BFFs give simpler security isolation per portal; AppSync adds resolver complexity for two well-defined clients |
| Saga choreography (EventBridge) | Step Functions orchestration | Choreography is more loosely coupled; Step Functions adds a central orchestrator that can become a single point of failure |
| Amazon Cognito | Auth0 / Okta | Cognito stays within the AWS ecosystem, reducing latency and IAM integration complexity; lower cost at scale |
| AWS App Mesh | Istio on EKS | App Mesh has native AWS integration (ACM, CloudMap, X-Ray); Istio is more feature-rich but operationally heavier |
| RDS PostgreSQL | Amazon Aurora Serverless | RDS is predictable cost and performance at known load; Aurora Serverless better for unpredictable bursty traffic |
| EKS | ECS Fargate | EKS provides more control for service mesh, network policies, and future platform evolution |

### Recommendations

1. **API Versioning from Day 1**: Use URL versioning (`/v1/`, `/v2/`) in BFFs to allow client and server to evolve independently without coordinated deployments.

2. **Consumer-Driven Contract Testing**: Implement Pact contract tests between BFFs and microservices in CI to catch breaking API changes before they reach staging.

3. **AWS FIS Chaos Engineering**: Schedule quarterly chaos experiments (AZ failure, dependency latency injection) to validate circuit breakers and fallbacks under realistic failure conditions.

4. **PII Data Minimisation Review**: Conduct quarterly reviews of what PII is collected vs. strictly necessary. Use Amazon Macie findings as input. Excess PII collection is a compliance liability under Singapore PDPA.

5. **Payment Reconciliation Job**: Implement a nightly AWS Lambda or ECS task that compares Payment Service RDS records against Stripe's transaction log API to catch discrepancies. Alert on any mismatch.

6. **Agent Portal IP Restriction**: Consider AWS WAF IP set rules at CloudFront/API Gateway to restrict the Agent BFF to known corporate/VPN IP ranges. Agents operate from known networks — this significantly reduces the attack surface at near-zero cost.

7. **Cost Optimisation**: Use Savings Plans for steady-state EKS node groups; Spot Instances for non-payment workloads (Quotation Service, Customer BFF); reserve RDS instances (1-year). Payment Service on On-Demand only (reliability over cost).

---

*Document version: 1.0 (AWS Edition) | Author: Solution Architect | Date: 2026-04-19*
