import { Component, EventEmitter, inject, input, Input, OnInit, output, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { JsonPipe, NgIf } from '@angular/common';
import { BsDatepickerConfig, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, JsonPipe, NgIf, BsDatepickerModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {

  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  registerCancel = output<boolean>();
  registerForm: FormGroup = new FormGroup({});
  bsConfig: Partial<BsDatepickerConfig> = {
      containerClass: 'theme-dark-blue',
      dateInputFormat: 'DD/MM/YYYY',
  };

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],     //prvi parametar je inicijalna vrednost
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: true}
    }
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({dateOfBirth: dob});   //ovako updatujemo polje u register 
    this.accountService.register(this.registerForm.value).subscribe({
      next: respone => this.router.navigateByUrl('/members'),
      error: error => this.toastr.error(error.error)
    })
  }

  cancel() {
    this.registerCancel.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    return new Date(dob).toISOString().slice(0,10); //konvertuje datum u standardni ISO format: "YYYY-MM-DDTHH:mm:ss.sssZ",uzima samo prvih 10 karaktera (YYYY-MM-DD)
  }
}
