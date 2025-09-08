using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.Services
{
    public interface ICorrelationIdService
    {
        string GetCorrelationId();
        void SetCorrelationId(string correlationId);
        string GenerateNewCorrelationId();
    }
}
