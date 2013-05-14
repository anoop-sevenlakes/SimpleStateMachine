workflow "AFE Approval"

trigger @Submit
trigger @Accept
trigger @Reject
trigger @Approve
trigger @Publish

state @New:
	when @Submit      > @PendingReview
		on_event @AfeSubmitTask
	
state @PendingReview:
	when @Accept		> @PendingReview
		on_event @AfeAcceptTask
	when @LastAccept		> @PendingApproval 
		on_event @AfeAcceptTask
	when @Reject        > @New
		on_event @AfeRejectTask
	
state @PendingApproval:
	when @Accept     > @PendingApproval
		on_event @AfeAcceptTask
	when @LastAccept     > @Approved
		on_event @AfeAcceptTask
	when @Reject         > @New
		on_event @AfeRejectTask

state @Approved:
	when @Publish       > @Published
		on_event @AfePublishTask

state @Published
