using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevFreela.Application.ViewModels;
using MediatR;

namespace DevFreela.Application.Queries.GetUser
{
    public class GetUserById : IRequest<UserViewModel>
    {
        public GetUserById(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}