import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import { AuthService } from '../../../services/auth/auth';
import { RegisterModel } from '../../../models/user';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})

export class Register implements OnInit {
  form!: FormGroup;
  isRegistering: boolean = false;


  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router)
  {
  }

  ngOnInit(): void {
    this.form  = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
    }

  onSubmit() {
    if (this.form.invalid || this.isRegistering) return;
    this.isRegistering = true;

    const dto: RegisterModel = this.form.value;
    this.auth.register(dto).subscribe({
      next: user => {
        // Handle success (e.g. redirect or show message)
        this.isRegistering = false;
        console.log('Registered:', user);
        this.router.navigate(['/dashboard']);
      },
      error: err => {
        // Handle error
        this.isRegistering = false;
        console.error('Registration failed:', err);
      }
    });
  }
}
