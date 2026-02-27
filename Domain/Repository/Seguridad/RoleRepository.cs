using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.seguridad;
using Domain.Interfaces.Seguridad;
using Domain.Repository.Base;

namespace Domain.Repository.Seguridad
{
    public class RoleRepository(SqlDbContext sqlDbContext) : Repository<Role>(sqlDbContext), IRoleRepository
    {

    }
}