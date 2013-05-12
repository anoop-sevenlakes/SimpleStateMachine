workflow "AFE Approval"

trigger @Submit
trigger @Accept
trigger @Reject
trigger @Approve
trigger @Publish

state @New:
	when @Submit      > @PendingReview
	
state @PendingReview:
	when @Accept By Reviewers,Approvers then @PendingApproval 
	when @Reject        > @New
	
state @PendingApproval:
	when @Approve     > @Approved
	when @Reject         > @New

state @Approved:
	when @Publish       > @Published

state @Published