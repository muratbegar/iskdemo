using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.Mappers;
using ELearningIskoop.Users.Domain.Repos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.UpdateUserProfile
{
    internal class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand, UserDTO>
    {

        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository,IUnitOfWork unitOfWork,ILogger<UpdateUserProfileCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDTO> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                throw new EntityNotFoundException("User", request.UserId);
            }

            PersonName? name = null;
            if (!string.IsNullOrWhiteSpace(request.FirstName) && !string.IsNullOrWhiteSpace(request.LastName))
            {
                name = new PersonName(request.FirstName, request.LastName);
            }


            user.UpdateProfile(
                name,
                request.Bio,
                request.PhoneNumber,
                request.ProfilePictureUrl,
                request.RequestedBy?.ToString()
            );

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User profile updated: {UserId}", user.ObjectId);

            return UserMapper.ToDTO(user);


        }
    }
}
