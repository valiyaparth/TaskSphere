﻿using TaskSphere.Models;
using TaskSphere.Data;
using TaskSphere.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace TaskSphere.Repository
{
    public class TaskUserRepository : ITaskUserRepository
    {
        private ApplicationDbContext _db;

        public TaskUserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async System.Threading.Tasks.Task AssignUserToTaskAsync(TaskUser taskUser)
        {
            await _db.TaskUsers.AddAsync(taskUser);
        }

        public async System.Threading.Tasks.Task RemoveUserFromTaskAsync(TaskUser taskUser)
        { 
            if (taskUser != null)
            {
                 _db.TaskUsers.Remove(taskUser);
            }
        }
    }
}
