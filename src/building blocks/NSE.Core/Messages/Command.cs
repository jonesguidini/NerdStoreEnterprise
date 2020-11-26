using FluentValidation.Results;
using MediatR;
using System;

namespace NSE.Core.Messages
{
    public abstract class Command : Message, IRequest<ValidationResult>
    {
        public DateTime TimeStamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        public Command()
        {
            TimeStamp = DateTime.Now;
        }

        public virtual bool EhValido()
        {
            // caso a classe q herdar de comando não implementar (override) este comando retornará essa exception
            throw new NotImplementedException();
        }

    }
}
