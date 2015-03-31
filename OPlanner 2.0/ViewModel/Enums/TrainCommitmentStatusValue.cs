
namespace PlannerNameSpace
{
    public enum TrainCommitmentStatusValue
    {
        NotCalculated,
        NotCommitted,
        CommittedNotApproved,
        CommittedAndApproved,
        Completed,
        ProjectedPastDue,
        PastDue,
        MovedToLaterTrain,
        MovedToEarlierTrain,
        AssignedToFutureTrain,
        CarriedOverFromPreviousTrain,
        OnTrack,
    }
}
