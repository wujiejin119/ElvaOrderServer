﻿using ElvaOrderServer.Domain.Entities;

namespace ElvaOrderServer.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);
        Task AddAsync(Order order);
    }
}
