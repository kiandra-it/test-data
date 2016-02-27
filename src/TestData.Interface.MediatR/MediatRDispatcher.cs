using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace TestData.Interface.MediatR
{
    public class MediatRDispatcher: IDispatcher
    {
        private readonly IMediator _mediator;
        public MediatRDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<IEnumerable<string>> DispatchAsync(IDataSetRequest request)
        {
            var newRequest = new DataSetRequest
            {
                DataSet = request.DataSet,
                Properties = request.Properties
            };

            return _mediator.SendAsync(newRequest);
        }
    }
}
