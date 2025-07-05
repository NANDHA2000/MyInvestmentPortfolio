import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { URL_LIST } from '../Config/url.config';



@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css'],
})
export class LandingPageComponent {
  features: any;

  constructor(private http: HttpClient,private router: Router) {}

  ngOnInit(): void {
    this.getFeaturesData();
  }

getFeaturesData() {
  this.http.get<any[]>(URL_LIST.FEATURES).subscribe((res) => {
    this.features = res.map(item => ({
      label: item.FeatureName,
      route: item.Route ? item.Route : '/' + item.FeatureName.toLowerCase().replace(/\s+/g, '-')
    }));
    console.log(this.features);
  });
}


  logout() {
    localStorage.removeItem('userToken');
    this.router.navigate(['/login']);
  }
}
