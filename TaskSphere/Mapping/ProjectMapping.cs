using AutoMapper;
using TaskSphere.DTOs;
using TaskSphere.Models;

namespace TaskSphere.Mapping
{
    public class ProjectMapping : Profile
    {
        public ProjectMapping() 
        {
            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectDto, Project>();

            CreateMap<ProjectMember, ProjectMemberDto>();
            CreateMap<ProjectMemberDto, ProjectMember>();

            CreateMap<ProjectTeam, ProjectTeamDto>();
            CreateMap<ProjectTeamDto, ProjectTeam>();

            CreateMap<Models.Task, GetTaskDto>();
            CreateMap<GetTaskDto, Models.Task>();
        }
    }
}
