/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { UsersState } from '@app/features/administration/internal';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class UnsetUserGuard implements CanActivate {
    constructor(
        private readonly usersState: UsersState
    ) {
    }

    public canActivate(): Observable<boolean> {
        return this.usersState.select(null).pipe(map(u => u === null));
    }
}