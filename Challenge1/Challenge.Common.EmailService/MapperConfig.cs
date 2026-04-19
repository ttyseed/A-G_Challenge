using AutoMapper;
using challenge1.Common.EmailService.Classes;
using challenge1.Common.EmailService.DTOs;
using challenge1.Common.EmailService.Models;

namespace challenge1.Common.EmailService
{
    internal class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<GetPendingBatchEmailsResponseDTO, Email>().ReverseMap();
            CreateMap<AttachmentDTO, EmailAttachment>().ReverseMap();
            CreateMap<SendManualEmailRequestDTO, Email>().ReverseMap();
            CreateMap<SendManualEmailResponseDTO, Email>().ReverseMap();
            CreateMap<AddAttachmentRequestDTO, EmailAttachment>().ReverseMap();
            CreateMap<EmailAttachment, AddAttachmentResponseDTO>().ReverseMap();
            CreateMap<AttachmentDTO, EmailAttachment>()
                .ForMember(dest => dest.AttachmentType,
                    opt => opt.MapFrom(src => Constant.MapAttachmentType(src.AttachmentType)));
            CreateMap<EmailAttachment, AttachmentDTO>()
                .ForMember(dest => dest.AttachmentType,
                    opt => opt.MapFrom(src => Constant.MapAttachmentTypeEnum(src.AttachmentType)));


        }
    }
}
