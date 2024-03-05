import { Component, OnInit } from '@angular/core';
import { AuthserviceService } from '../auth/authservice.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
  isDropdownOpen: boolean = false; // Initial state of the dropdown


  constructor(private authService: AuthserviceService) {} // Inject the authentication service
  
  ngOnInit(): void {
    const userExists = this.authService;
    console.log(userExists)
  }

  logout(): void {
    this.authService.logout(); // Call the logout method from the authentication service
  }

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen; // Toggle the dropdown state
  }
}
