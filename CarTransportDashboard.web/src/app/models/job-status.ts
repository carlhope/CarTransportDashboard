export enum JobStatus {
  Available = 'Available',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Allocated = 'Allocated',
}

export const JobStatusDisplay: Record<JobStatus, string> = {
  [JobStatus.Available]: 'Available',
  [JobStatus.InProgress]: 'In Progress',
  [JobStatus.Completed]: 'Completed',
  [JobStatus.Allocated]: 'Allocated',
};
