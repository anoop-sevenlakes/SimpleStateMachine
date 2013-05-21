workflow "AFEApproval"  
trigger @SUBMIT 
trigger @ACCEPT 
trigger @LASTACCEPT 
trigger @REJECT 
trigger @PUBLISH 
  
state @DRFT: 
 when @SUBMIT > @PAPRVL 
   on_event @CreateTask 

state @PAPRVL: 
 when @ACCEPT > @PAPRVL 
   on_event @CreateTask 
 when @LASTACCEPT > @PAAPRVL 
   on_event @CreateTask 
 when @REJECT > @DRFT 
   on_event @CreateTask 

state @PAAPRVL: 
 when @ACCEPT > @PAAPRVL 
   on_event @CreateTask 
 when @LASTACCEPT > @APRVD 
   on_event @CreateTask 
 when @REJECT > @DRFT 
   on_event @CreateTask 

state @APRVD: 
 when @PUBLISH > @PBLSD 
   on_event @CreateTask 
  
state @PBLSD
