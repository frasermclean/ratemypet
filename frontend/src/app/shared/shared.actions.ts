export namespace SharedActions {
  export class SetPageTitle {
    static readonly type = '[Shared] Set Page Title';
    constructor(public title: string, public url: string) {}
  }
}
