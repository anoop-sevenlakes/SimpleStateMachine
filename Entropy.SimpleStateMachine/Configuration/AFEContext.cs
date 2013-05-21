using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entropy.SimpleStateMachine.Configuration
{
    public class AFEContext
    {
        public int UserID = 1;
        public int AFEID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public Guid WorkflowID { get; set; }

        public List<AFEAssignment> Assignments { get; set; }

        public bool IsLastUserInTheCurrentOrder(string roleCode)
        {
            AFEAssignment assignment = this.Assignments.Find(t => t.AfeRoleCode == roleCode && t.UserID == this.UserID);
            if(assignment != null)
            {
                int currentOrder = assignment.Order;
                return IsLastUserInTheCurrentOrder(currentOrder);
            }
            else
                return false;
        }
        public bool IsLastUserInTheCurrentOrder(int currentOrder)
        {
            if (this.Assignments.FindAll(t => t.Order <= currentOrder && t.AFEActivityID == null).Count == 1)
                return true;
            else
                return false;

        }
        public bool IsLastPersonInTheCurrentRole(string roleCode)
        {
            if (this.Assignments.FindAll(t => t.AfeRoleCode == roleCode && t.AFEActivityID == null).Count == 1
                                && this.Assignments.FindAll(t => t.AfeRoleCode == roleCode && t.UserID == this.UserID && t.AFEActivityID == null).Count == 1)
                return true;
            else
                return false;
        }

    }
    public class AFEAssignment
    {

        public string AfeRoleCode { get; set; }
        public string AfeRoleName { get; set; }
        public int? AFETaskID { get; set; } //true? 0 : (int?) null;
        public int? AFEActivityID { get; set; } //true? 0 : (int?) null;
        public int UserID { get; set; }
        public int Order { get; set; }

        
    }
}
