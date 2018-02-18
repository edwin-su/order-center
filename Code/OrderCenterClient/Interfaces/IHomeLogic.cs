using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCenterClient.Interfaces
{
    public interface IHomeLogic
    {
        bool Login(string userName, string password);

        bool AddUser(string name, int age);
    }
}
