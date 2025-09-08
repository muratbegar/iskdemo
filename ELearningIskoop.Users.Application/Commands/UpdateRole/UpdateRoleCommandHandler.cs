using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Application.Commands.CreateRole;
using ELearningIskoop.Users.Application.Services;
using ELearningIskoop.Users.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        public UpdateRoleCommandHandler(IRoleManager roleManager, IUnitOfWork unitOfWork, ILogger<UpdateRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Domain service ile role güncelle (business rules çalışır)
                await _roleManager.UpdateRoleAsync(request.RoleId, request.Name, request.Description);

                // Unit of Work ile commit
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
            
        }
    }
}
