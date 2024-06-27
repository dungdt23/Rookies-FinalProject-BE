using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Domain.Constants
{
	public static class AssignmentApiResponseMessageConstant
	{
		public const string AssignmentGetSuccess = "Assignment(s) found";
		public const string AssignmentGetNotFound = "No assignment record found";
		public const string AssignmentCreateSuccess = "Assignment created successfully";
		public const string AssignmentCreateFail = "There something went wrong while creating assignment, please try again later";
		public const string AssignmentUpdateSuccess = "Assignment updated successfully";
		public const string AssignmentUpdateFail = "There something went wrong while updating assignment, please try again later";
		public const string AssignmentNotFound = "No assignment found";
		public const string AssignmentDeleteStateConfict = "Only assignment waiting for acceptance or declined can be deleted";
		public const string AssignmentDeleteSuccess = "Assignment deleted successfully";
		public const string AssignmentDeleteFail = "There something went wrong while deleting assignment, please try again later";
		public const string AssignmentRespondSuccess = "Assignment responded successfully";
		public const string AssignmentRespondFail = "There something went wrong while responding to assignment, please try again later";
		public const string AssignmentRespondNotWaitingForAcceptance = "Only assignment waiting for acceptance can be responded";
		public const string AssignmentRespondNotAvailable = "Only asset assigned to assignment is available can be responded";
		public const string AssetNotFound ="Asset not found";
		public const string AssetNotAvailable = "Asset not available";

	}
}
