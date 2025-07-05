import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { URL_LIST } from '../Config/url.config';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-mutual-fund',
  templateUrl: './mutual-fund.component.html',
  styleUrls: ['./mutual-fund.component.css'],
})
export class MutualFundComponent implements OnInit {
  selectedFile: File | null = null;
  mutualFundData: any[] = [];
  schemePerformanceData: any[] = []; // For the second API response
  baseFileName: string | undefined;
  showPerformanceTable: boolean = false;
  schemeNames: string[] = []; // Array to hold the scheme names
  selectedScheme: string = ''; // To hold the selected scheme
  // schemeNames: string[] = [
  //   "Quant Small Cap Fund Direct Plan Growth",
  //   "ICICI Prudential Infrastructure Direct Growth",
  //   "Parag Parikh Flexi Cap Fund Direct Growth"
  // ];

  constructor(
    private http: HttpClient,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    //this.triggerDayPerformance();
    this.getInvestmentData();
    //this.getSchemeNames();
  }

  onFileSelect(event: any): void {
    this.selectedFile = event.target.files[0];
    console.log(this.selectedFile);
    const fileName = this.selectedFile?.name;
    // Use a regular expression to remove the date and extension, leaving only the base name
    this.baseFileName = fileName?.split('_').slice(0, 3).join('_');
  }

  onUpload() {
    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('file', this.selectedFile);

      this.http.post<any>(URL_LIST.ADD_MF_DETAILS, formData).subscribe(
        (res) => {
          if (res.success === true) {
            this.toastr.success('File uploaded successfully!', 'Success');
            this.getInvestmentData(); // Refresh data after upload
          } else {
            this.toastr.error('Issue in uploading! Please try again.', 'Error');
          }
        },
        (error) => {
          this.toastr.error('Server Error! Please try later.', 'Error');
        }
      );
    }
  }

  getInvestmentData() {
    this.http.get<any[]>(`${URL_LIST.GET_MF_DETAILS}`).subscribe((res) => {
      console.log(res);

      this.mutualFundData = res;
    });
  }

  getSchemeNames() {
    this.http.get<any[]>(`${URL_LIST.GET_SCHEME_NAMES}`).subscribe((res) => {
      this.schemeNames = res;
      console.log(this.schemeNames);
    });
  }

  viewDayPerformance(data: any) {
    debugger;
    console.log(data.schemeName);

    this.getDayPerformance(data.schemeName);
    this.showPerformanceTable = true;
  }

  getDayPerformance(schemeName: string) {
    debugger;
    this.http
      .get<any[]>(
        `${
          URL_LIST.GET_MF_DAYPERFORMANCE_DETAILS
        }/?schemeName=${encodeURIComponent(schemeName)}`
      )
      .subscribe((res) => {
        this.schemePerformanceData = res;
        console.log(res);

        console.log(this.schemePerformanceData);
      });
  }

  triggerDayPerformance() {
    this.http
      .get<any[]>(`${URL_LIST.TRIGGER_MF_DAYPERFORMANCE_DETAILS}`)
      .subscribe((res) => {
        console.log(res);
      });
  }

  selectScheme(scheme: string): void {
    this.selectedScheme = scheme;
    console.log('Selected Scheme:', this.selectedScheme);
  }

  onSchemeNameClick(schemeName: string) {
    console.log('Scheme Name clicked:', schemeName);

    // Store the selected scheme name
    // this.selectedSchemeName = schemeName;

    // // Make an API call to get the scheme performance data based on the schemeName
    // this.http.get(`your-endpoint-for-scheme-performance/${schemeName}`).subscribe((data: any) => {
    //   // Handle the response, you can store it in a variable and display it
    //   console.log('Scheme Performance Data:', data);
    //   // You can assign the response data to a variable to display in a table or another view
    //   this.schemePerformanceData = data;
    // });
  }

  goBack(): void {
    this.router.navigate(['/landingpage']); // Adjust this to your desired previous route
  }
}
