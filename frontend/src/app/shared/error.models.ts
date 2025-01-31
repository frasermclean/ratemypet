export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  instance: string;
  traceId: string;
  detail?: string;
  errors: ProblemDetailsError[];
}

export interface ProblemDetailsError {
  name: string;
  reason: string;
}
