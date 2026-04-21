import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth/auth';
import { LoginModel } from '../../../models/user';
import { HttpClient } from '@angular/common/http';

declare const google: any;

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {
  form!: FormGroup;
  isLoggingIn:boolean = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }
  ngAfterViewInit(): void {
    const checkGoogle = setInterval(() => {
      if (window.hasOwnProperty('google')) {
        clearInterval(checkGoogle);

        google.accounts.id.initialize({
          client_id: '561971564563-18a63e1t8e9c03oi947v4a014fr22qh3.apps.googleusercontent.com',
          callback: (response: any) => this.handleGoogleResponse(response)
        });

        google.accounts.id.renderButton(
          document.getElementById('googleBtn'),
          { theme: 'outline', size: 'large' }
        );
      }
    }, 100);
  }

  onSubmit() {
    if (this.form.invalid || this.isLoggingIn) return;
    this.isLoggingIn = true;

    const dto: LoginModel = this.form.value;
    this.auth.login(dto).subscribe({
      next: user => {
        this.isLoggingIn = false;
        console.log('Logged in:', user);
        this.router.navigate(['/dashboard']);
      },
      error: err => {
        this.isLoggingIn = false;
        console.error('Login failed:', err);
      }
    });
  }

  handleGoogleResponse(response: any) {
    if(this.isLoggingIn) return;
    this.isLoggingIn = true;
    const idToken = response.credential;

    this.auth.googleLogin(idToken).subscribe({
      next: user => {
        this.isLoggingIn = false;
        console.log('Logged in via Google:', user);
        this.router.navigate(['/dashboard']);
      },
      error: err => {
        this.isLoggingIn = false;
        console.error('Google login failed:', err)
      }
    });
  }
}

