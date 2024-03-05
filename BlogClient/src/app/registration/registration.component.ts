import { Component } from '@angular/core';
import { Useregisteration } from '../auth/useregisteration';
import { AuthserviceService } from '../auth/authservice.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {
  user: Useregisteration = new Useregisteration();

  constructor(private authserviceService: AuthserviceService) { }

  registerUser() {
    this.authserviceService.RegistrationUser(this.user)
      .subscribe(
        response => {
          console.log('User registered successfully:', response);
          alert('successful Registered User!');

        },
        error => {
          console.error('Error during user registration:', error);
        }
      );
  }
}
