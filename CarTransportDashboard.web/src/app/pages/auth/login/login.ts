import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { AuthService } from '../../../services/auth/auth';
import { LoginModel } from '../../../models/user';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {

  private form!: FormGroup;

  constructor(private fb: FormBuilder, private auth: AuthService) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    const dto: LoginModel = this.form.value;
    this.auth.login(dto).subscribe({
      next: user => {
        localStorage.setItem('accessToken', user.refreshToken ?? '');
        console.log('Logged in:', user);
      },
      error: err => {
        console.error('Login failed:', err);
      }
    });
  }
}

