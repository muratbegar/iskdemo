using ELearningIskoop.BuildingBlocks.Application.Behaviors;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.BuildingBlocks.Application.Services;
using ELearningIskoop.BuildingBlocks.Infrastructure.BackgroundServices;
using ELearningIskoop.BuildingBlocks.Infrastructure.Outbox;
using ELearningIskoop.Shared.Infrastructure.Outbox;
using ELearningIskoop.Users.Application.Commands.CreateRole;
using ELearningIskoop.Users.Application.Commands.DeleteRole;
using ELearningIskoop.Users.Application.Commands.UpdateRole;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.DTOs.Role;
using ELearningIskoop.Users.Application.EventHandlers;
using ELearningIskoop.Users.Application.Queries.GetAllRoles;
using ELearningIskoop.Users.Application.Queries.GetRoleById;
using ELearningIskoop.Users.Application.Queries.SearchRoles;
using ELearningIskoop.Users.Application.Services;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.EventList;
using ELearningIskoop.Users.Domain.EventList.Role;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUsersApplication(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
            // Register FluentValidation
            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

            // Register Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            //Role Pipeline Behaviors
            services.AddScoped<IValidator<CreateRoleCommand>, CreateRoleCommandValidator>();
            services.AddScoped<IValidator<UpdateRoleCommand>, UpdateRoleCommandValidator>();
            services.AddScoped<IValidator<DeleteRoleCommand>, DeleteRoleCommandValidator>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICorrelationIdService, CorrelationIdService>();


            // Outbox services
            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddScoped<IOutboxService, OutboxService>();

            // Background service for processing outbox events
            services.AddHostedService<OutboxEventProcessor>();


            // Register Services
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailServices, EmailServices>();
            services.AddScoped<IRoleManager,RoleManager>();
            //services.AddScoped<ILoginHistoryService, LoginHistoryService>();

            // Role Command Handlers
            services.AddScoped<IRequestHandler<CreateRoleCommand, Result<Role>>, CreateRoleCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateRoleCommand, Result>, UpdateRoleCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteRoleCommand, Result>, DeleteRoleCommandHandler>();

            // Role Query Handlers
            services.AddScoped<IRequestHandler<GetRoleIdQuery, RoleDto>, GetRoleByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllRolesQuery, PagedResult<RoleListDto>>, GetAllRolesQueryHandler>();
            services.AddScoped<IRequestHandler<SearchRolesQuery, PagedRoleResult>, SearchRolesQueryHandler>();


            // Register Event Handlers
            services.AddScoped<INotificationHandler<UserCreatedDomainEvent>, UserCreatedDomainEventHandler>();
            services.AddScoped<INotificationHandler<UserEmailVerifiedDomainEvent>, UserEmailVerifiedDomainEventHandler>();
            services.AddScoped<INotificationHandler<RoleCreatedEvent>, RoleCreatedEventHandler>();
            
            //services.AddScoped<INotificationHandler<UserLockedDomainEvent>, UserLockedDomainEventHandler>();
            //services.AddScoped<INotificationHandler<UserPasswordChangedDomainEvent>, UserPasswordChangedDomainEventHandler>();
            //services.AddScoped<INotificationHandler<UserRoleAssignedDomainEvent>, UserRoleAssignedDomainEventHandler>();

            return services;
        }
    }
}
