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

namespace ELearningIskoop.Users.Application.Commands.DeleteRole
{

    internal class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;
        public DeleteRoleCommandHandler(IRoleManager roleManager, IUnitOfWork unitOfWork, ILogger<DeleteRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {

            try
            {
                // Domain service ile role sil (business rules çalışır)
                await _roleManager.DeleteRoleAsync(request.RoleId);
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
