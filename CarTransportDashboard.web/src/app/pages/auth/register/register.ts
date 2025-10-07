import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
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


  constructor(private fb: FormBuilder, private auth: AuthService)
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
    if (this.form.invalid) return;

    const dto: RegisterModel = this.form.value;
    this.auth.register(dto).subscribe({
      next: user => {
        // Handle success (e.g. redirect or show message)
        console.log('Registered:', user);
      },
      error: err => {
        // Handle error
        console.error('Registration failed:', err);
      }
    });
  }
}
