export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  instance: string;
  traceId: string;
  errors: ProblemDetailsError[];
}

export interface ProblemDetailsError {
  name: string;
  reason: string;
}
