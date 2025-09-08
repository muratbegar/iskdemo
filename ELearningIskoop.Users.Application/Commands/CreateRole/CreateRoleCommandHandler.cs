using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.BuildingBlocks.Application.Services;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.BuildingBlocks.Infrastructure.Outbox;
using ELearningIskoop.Users.Application.Services;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.EventList.Role;
using ELearningIskoop.Users.Domain.Repos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ELearningIskoop.Users.Application.Commands.CreateRole
{
    //MediatR Pattern: IRequestHandler<TRequest, TResponse> implementasyonu
    //Dependency Injection: Domain service ve UnitOfWork injection
    //Error Handling: Specific exceptions catch edilip user-friendly response'a çevriliyor
    //Transaction Management: UnitOfWork ile commit
    //Single Responsibility: Sadece CreateRole command'ını handle eder

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand,Result<Role>>
    {
        private readonly IRoleManager _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICorrelationIdService _correlationIdService;


        private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(IRoleManager roleManager,IUnitOfWork unitOfWork, IOutboxService outboxService,ICurrentUserService currentUserService,ICorrelationIdService correlationIdService, ILogger<CreateRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _outboxService = outboxService;
            _currentUserService = currentUserService;
            _correlationIdService = correlationIdService;
            _logger = logger;
        }

        public async Task<Result<Role>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Domain servis ile role oluştur
                var role = await _roleManager.CreateRoleAsync(request.RoleName, request.Description,
                    Enumerable.Empty<string>());

                await _unitOfWork.SaveChangesAsync(cancellationToken);


                // Domain event'i shared outbox'a ekle
                var domainEvent = new RoleCreatedEvent(
                    role.Value.ObjectId,
                    role.Value.Name,
                    _currentUserService.GetCurrentUserId()
                );

                await _outboxService.PublishAsync(
                    domainEvent: domainEvent,
                    aggregateId: role.Value.ObjectId.ToString(),
                    aggregateType: "Role",
                    moduleName: "Users",
                    userId: _currentUserService.GetCurrentUserId(),
                    correlationId: _correlationIdService.GetCorrelationId(),
                    priority: 0 // Normal priority
                );

                return role;

            }
            catch (ArgumentException ex)
            {
                return Result.Failure<Role>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<Role>(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure<Role>(ex.Message);
            }
           
        }
    }
}
