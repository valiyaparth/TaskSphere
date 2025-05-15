using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskSphere.Data;

namespace TaskSphere.Authorization
{
    public class adminHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;

        public adminHandler(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                context.Fail();
                return;
            }

            var routeData = _contextAccessor.HttpContext?.Request.RouteValues;
            if (routeData == null)
            {
                context.Fail();
                return;
            }

            int projectId = 0, teamId = 0, taskId = 0;

            // Check if the route data contains the projectId / teamId / taskId
            bool hasProjectId = routeData.ContainsKey("projectId") 
                && int.TryParse(routeData["projectId"]?.ToString(), out projectId);
            bool hasTeamId = routeData.ContainsKey("teamId")
                && int.TryParse(routeData["teamId"]?.ToString(), out teamId);
            bool hasTaskId = routeData.ContainsKey("taskId")
                && int.TryParse(routeData["taskId"]?.ToString(), out taskId);

            //check for admin role
            bool isProjectAdmin = hasProjectId && await _db.ProjectMembers
                .AnyAsync(pm => pm.UserId == userId && pm.ProjectId == projectId && pm.Role == Enums.Roles.Admin);

            var isTeamAdmin = hasTeamId && await _db.TeamMembers
                .AnyAsync(tm => tm.UserId == userId && tm.TeamId == teamId && tm.Role == Enums.Roles.Admin);

            var isTaskAdmin = hasTaskId && await _db.TaskUsers
                .AnyAsync(tu => tu.UserId == userId && tu.TaskId == taskId && tu.Role == Enums.Roles.Admin);

            if (isProjectAdmin || isTeamAdmin || isTaskAdmin)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
