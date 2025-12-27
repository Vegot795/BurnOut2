using Infrastructure.Data;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services

{
    public class TerminsService
    {
        private readonly ApplicationDbContext _db;
        public TerminsService(ApplicationDbContext db) => _db = db;
       
       
    }
}
