import { Component } from '@angular/core';
import { Useregisteration } from '../auth/useregisteration';
import { AuthserviceService } from '../auth/authservice.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  user: Useregisteration = new Useregisteration();

  constructor(private authService: AuthserviceService) {}

  login(): void {
    this.authService.login(this.user)
      .subscribe(
        (response) => {
          console.log(response)
          // this.router.navigate(['/post']);

        },
        (error) => {
          // Handle login error, e.g., display error message
          console.error('Login failed:', error);
        }
      );
  }
}
