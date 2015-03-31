
namespace PlannerNameSpace
{
    public static class Constants
    {
        public const string PillarFriendlyName = "Area";
        public const string TrainFriendlyName = "Train";
        public const string BacklogFriendlyName = "Feature";
        public const string BacklogsFriendlyName = BacklogFriendlyName + "s";
        public const string ScenarioFriendlyName = "Experience";
        public const string ScenariosFriendlyName = ScenarioFriendlyName + "s";
        public const string ScenarioOwnerFriendlyName = ScenarioFriendlyName + " Owner";
        public const string ScenarioFeaturesFriendlyName = BacklogsFriendlyName + " Assigned to this " + ScenarioFriendlyName;
        public const int MaxShortStringLength = 100;
        public const double AvgCapacityPerDay = 6;
        public const int IdealHoursPerDay = 8;
        public const int BugBufferDaysBetweenBacklogItems = 5;
        public const string c_NotSet = "<Not Set>";
        public const string c_All = "<All>";
        public const string c_Any = "<Any>";
        public const string c_NoneSpecTeamName = "None";
        public const string c_None = "<None>";
        public const string c_ExternalTeam = "<External Team>";
        public const string c_noSpecRequired = "(No Spec Required)";
        public const string c_SpecTBD = "(Spec Location TBD)";
        public const string c_SpecdInBacklog = "(Spec'd in Backlog)";
        public const string c_AssignedToFutureTrain = "Assigned to a Future Train";
        public const string OfficeCurrentShipCycle = "Gemini";
        public const string OfficeBacklogFixBy = "TBD (Product Backlog)";
        public const string AcceptanceCriteriaFileName = "AcceptanceCriteria.rtf";
        public const string DescriptionFileName = "Description.rtf";
        public const string PSErrIssueUpdated = "The issue has been updated. Reopen the issue and reapply your changes";
        public const string PSErrAttachmentShare = "The middle tier cannot access the permanent attachment share.";
        public const string PSErrExecuteFailedPleaseRetry = "Please retry the transaction";
        public const string CannotForecast = "Cannot Forecast";
        public const string GoStatus = "Current";
        public const string NoGoStatus = "Not Current";
        public const string CalculatingStatus = "(Calculating...)";
        public const string NotAssigned = "(Not Assigned)";
        public const string AlreadyCompleted = "(Completed)";
        public const string NotCommitted = "Not Committed";
        public const string CommittedNotApproved = "Ready for commitment, not yet approved";
        public const string CommittedAndApproved = "Committed and approved, not yet started";
        public const string NotScheduled = "(No work scheduled)";
        public const string NotInProgress = "(Not Started)";
        public const string DefaultScrumTeamName = "New Scrum Team";
        public const string CommitmentsApprovalNotDetermined = "(Determining commitment approval status...)";
        public const string CommitmentsApproved = "Commitments have been approved for the selected Pillar and Train.";
        public const string CommitmentsNotApproved = "Commitments have not yet been approved for the selected Pillar and Train.";
        public const string CommitmentsNotApplicable = "(Select a specific pillar and train to see commitment status)";

        public const string TrainCommitmentCompleted = "Commitment Completed";
        public const string TrainCommitmentOnTrack = "Commitment On Track";
        public const string TrainCommitmentPastDue = "Commitment Past Due";
        public const string TrainCommitmentAssignedToFutureTrain = c_AssignedToFutureTrain;
        public const string TrainCommitmentProjectedPastDue = "Commitment projected to finish late";
        public const string TrainCommitmentCompletedInLaterTrain = "Commitment was completed in a later train than the one it was originally committed to";
        public const string TrainCommitmentCompletedInEarlierTrain = "Commitment was completed in a train earlier than the one it was originally committed to";
        public const string TrainCommitmentChangedToLaterTrain = "Commitment has been moved to train later than the one it was originally committed to";
        public const string RecapTrainCommitmentChangedToLaterTrain = "Not Completed - work will continue during the next train";
        public const string TrainCommitmentChangedToEarlierTrain = "Commitment has been moved to train earlier than the one it was originally committed to";
        public const string TrainCommitmentCarriedOverFromPreviousTrain = "Carried over from previous train";
        public const string TrainCommitmentNewCommitment = "New Commitment";

        // Warning messages
        public const string ProductStudioMiddleTierProblems = "OPlanner is currently having difficulty communicating with Product Studio.  There may be some fields in your schedule that aren't populated, or cannot be changed. If you click Refresh, OPlanner will try again to read any missing data.";

        public const string ActiveStatusColor = "#33E53D10";
        public const string ResolvedStatusColor = "#33FFFF00";
        public const string ClosedStatusColor = "#3300FF00";
        public const string ChangesToSaveColor = "#33FF0000";

        public const string OpeningProductGroup = "Loading all your team's experiences and features...";
        // Tooltips
        public const string ColumnHeaderStoryPointsToolTip = "Story Points allow you to provide a rough early estimate for how much time (in team working days) your team feels it will take to complete a particular backlog item.\r\rIf you provide Story Points to an item that has been 'Committed', but is not yet 'In Progress', OPlanner will use that value to forecast a landing date for your backlog item. You can then use these estimated landing dates to help determine how many backlog items your team can commit to for a particular train.\r\rOnce your team has created all the work items that describe and estimate the work for a backlog item, and you set the Commitment Setting to 'In Progress', OPlanner will use the work item estimates from then on to calculate the landing date.";
        public const string ColumnHeaderCommitmentSettingToolTip = "The 'Commitment Setting' for a backlog item indicates whether your team or pillar has 'committed' to delivering the work described by the backlog item by the end of a specific train.  The possible values are:\r\rUncommitted: the team has not committed to working on this item yet.\r\rCommitted: the team has committed to fully finishing all the work described by the backlog item before the end of the train that the item is assigned to.  Note that you cannot set an item to 'Committed' until the spec for the item is in the 'Ready for Coding' state.\r\rIn Progress: the team has created all the work items that fully describe and estimate the work required to finish a backlog item, and is ready to (or has begun) the work.\r\rComplete: The work for the backlog item is fully complete, including all coding, unit testing, automation, and bug fixing.";
        public const string ColumnHeaderLandingDateToolTip = "The 'Landing Date' for a backlog item is OPlanner's estimate for when all the work for that item is expected to be fully completed, taking into account all the work items that your team has created to describe and estimate the work required.\r\rIf the backlog item is in the 'Committed' state (typically before detailed work items have been created), you can assign Story Points (in units of working days) to the item, in which case OPlanner will use that value as a rough estimate to calculate a landing date.";
        public const string LandingDateTooltipCompleted = "All work for this item has been completed";
    }
}
