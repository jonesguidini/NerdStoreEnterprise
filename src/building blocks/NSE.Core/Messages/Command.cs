using FluentValidation.Results;
using System;

namespace NSE.Core.Messages
{
    public abstract class Command : Message
    {
        public DateTime TimeStamp { get; private set; }
        public ValidationResult validationResult { get; set; }

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
