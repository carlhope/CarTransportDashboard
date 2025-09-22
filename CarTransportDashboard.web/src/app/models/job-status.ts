export enum JobStatus {
  Available,
  InProgress,
  Completed
}

export const JobStatusDisplay: Record<JobStatus, string> = {
  [JobStatus.Available]: 'Available',
  [JobStatus.InProgress]: 'In Progress',
  [JobStatus.Completed]: 'Completed'
};
