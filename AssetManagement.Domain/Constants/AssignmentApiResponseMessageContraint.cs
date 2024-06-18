using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Domain.Constants
{
	public static class AssignmentApiResponseMessageContraint
	{
		public const string AssignmentCreateSuccess = "Assignment created successfully";
		public const string AssignmentCreateFail = "There something went wrong while creating assignment, please try again later";
		public const string AssignmentNotFound = "No assignment found";
		public const string AssignmentDeleteNotWaitingForAcceptance = "Only assignment waiting for acceptance can be deleted";
		public const string AssignmentDeleteSuccess = "Assignment deleted successfully";
		public const string AssignmentDeleteFail = "There something went wrong while deleting assignment, please try again later";
		

	}
}
