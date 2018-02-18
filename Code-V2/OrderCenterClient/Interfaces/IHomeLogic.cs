using OrderCenterClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCenterClient.Interfaces
{
    public interface IHomeLogic
    {
        long Login(string userName, string password);

        bool CreateNewDefaultOperator(Operator operatorInfo, long operatorId);

        List<Operator> GetOperatorList();

        bool ChangeOperatorActiveStatus(Operator operatorInfo, long operatorId);

        bool UpdateOperator(Operator operatorInfo, long operatorId);

        bool DeleteOperator(Operator operatorInfo);

        OperatorPermission GetOperatorPermission();
    }
}
