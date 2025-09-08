using ELearningIskoop.Users.Domain.EventList.Role;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.EventHandlers
{
    public class RoleCreatedEventHandler : INotificationHandler<RoleCreatedEvent>
    {
        public async Task Handle(RoleCreatedEvent notification, CancellationToken cancellationToken)
        {

            // Event tetiklendiğinde yapılacak işlemler buraya
            // Örneğin: log, outbox kaydı, email vs.

            await Task.CompletedTask; // Şimdilik boş çalıştırmak için
        }
    }
}
