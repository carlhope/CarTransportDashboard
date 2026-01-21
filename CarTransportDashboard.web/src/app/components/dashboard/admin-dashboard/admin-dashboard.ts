import { Component } from '@angular/core';

@Component({
  selector: 'app-admin-dashboard',
  imports: [],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboard {
  //cancelJob(jobId: string): void {
    //cancelJob is admin only on backend. retained for re-use when admin dashboard created. To be replaced with requestCancellation
   // this.jobService.cancelJob(jobId).subscribe({
    //  next: updatedJob => {
     //   console.log(`Job ${jobId} cancelled.`);
     //   this.refreshData();
     // },
     // error: err => {
     //   console.error(`Failed to cancel job ${jobId}:`, err.message);
     // }
    //});
  //}
}
