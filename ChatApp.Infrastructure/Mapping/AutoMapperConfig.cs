using AutoMapper;
using ChatApp.Core.Entities;
using ChatApp.Core.Models;

namespace ChatApp.Infrastructure.Mapping;

public static class AutoMapperConfig
{
    public static IMapper Initialize()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ChatConversationEntity, ChatConversation>()
                .ForMember(
                dest => dest.ContactName,
                opt => opt.MapFrom(src => src.ContactName))
                .ForMember(
                dest => dest.ReceivedMessage,
                opt => opt.MapFrom(src => src.ReceivedMsgs))
                .ForMember(
                dest => dest.MsgReceivedOn,
                opt => opt.MapFrom(src => src.MsgReceivedOn))
                .ForMember(
                dest => dest.SentMessage,
                opt => opt.MapFrom(src => src.SentMsgs))
                .ForMember(
                dest => dest.MsgSentOn,
                opt => opt.MapFrom(src => src.MsgSentOn))
                .ForMember(
                dest => dest.IsMessageReceived,
                opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.ReceivedMsgs)))
                .ForMember(
                dest => dest.MessageContainsReply,
                opt => opt.MapFrom(src => src.IsReplied));
        });

        return config.CreateMapper();
    }
}