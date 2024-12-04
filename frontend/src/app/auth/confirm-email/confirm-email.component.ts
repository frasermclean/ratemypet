import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { dispatch } from '@ngxs/store';

import { AuthActions } from '../auth.actions';

@Component({
  template: ''
})
export class ConfirmEmailComponent implements OnInit {
  routeSnapshot = inject(ActivatedRoute);
  confirmEmail = dispatch(AuthActions.ConfirmEmail);

  ngOnInit(): void {
    const userId = this.routeSnapshot.snapshot.queryParamMap.get('userId');
    const token = this.routeSnapshot.snapshot.queryParamMap.get('token');
    if (!userId || !token) {
      throw new Error('Missing userId or token.');
    }

    this.confirmEmail(userId, token);
  }
}
