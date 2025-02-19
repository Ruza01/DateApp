using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services___Repos;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);

    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
    {
        var query = context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recepient.UserName == messageParams.Username
                && x.RecepientDeleted == false),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username 
                && x.SenderDeleted == false),
            _ => query.Where(x => x.Recepient.UserName == messageParams.Username && x.DateRead == null 
                && x.RecepientDeleted == false) //unread messages
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);   //ovo koristimo jer smo u AutoMapperConfiguration definisali CreateMap<Message,MessageDto>(), da nisam ovo napisao, radili bi sa Message

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

    }

    //sve poruke izmedju 2 korisnika
    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recepientUsername)
    {
        var messages = await context.Messages
            .Include(x => x.Sender).ThenInclude(x => x.Photos)
            .Include(x => x.Recepient).ThenInclude(x => x.Photos)
            .Where(
                x => x.RecepientUsername == currentUsername 
                    && x.RecepientDeleted == false 
                    && x.SenderUsername == recepientUsername ||
                x.SenderUsername == currentUsername 
                    && x.SenderDeleted == false 
                    && x.RecepientUsername == recepientUsername
            )
            .OrderByDescending(x => x.MessageSent)
            .ToListAsync();
        
        var unreadMessages = messages.Where(x => x.DateRead == null && x.RecepientUsername == currentUsername).ToList();

        if (unreadMessages.Count != 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }

        return mapper.Map<IEnumerable<MessageDto>>(messages);


    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
