using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace Intro;

public partial class MainWindow : Window
{
    private readonly ISlide[] _slides =
    [
        new ImageSlide("Images/logo.jpg", sizeToFit: true),
        new ImageSlide("Images/steps.png"),
        new ImageSlide("Images/ethos.jpg"),
        new TextSlide("Thanks to Patch", 30),
        new TextSlide("Gopher", 30),
        new TextSlide("Thanks to Patch", 30),
        
        // TIL In the 90s there existed an Internet Protocol called Gopher, developed by University of Minnesota,
        // that was initially more popular than the World Wide Web, died out because University of Minnesota decided
        // to charge fees to use the implementation of the Gopher server
        
        //
        // new ImageSlide("Images/early.jpg.webp", sizeToFit: true), // Harry Beck 1933
        // new ImageSlide("Images/london.gif"),
        // new ImageSlide("Images/new-york-map.png"),
        // new ImageSlide("Images/Tokyo_subway_map.png"),
        // new ImageSlide("Images/Singapore.png", 1600, 1200),
        // new ImageSlide("Images/liverpool.jpg"),
        // new TextSlide("What about York?", 30),
        // new ImageSlide("Images/york.png", sizeToFit: true),
        // new UrlSlide("https://content.tfl.gov.uk/tfl-line-diagram-standard.pdf", 50),
        // new ImageSlide("Images/SampleCode.png", sizeToFit: true),
        // new ImageSlide("Images/demo1.png", 1600, 1000),
        // new ImageSlide("Images/demo2.png", 1600, 1000),
        // new ImageSlide("Images/demo3.png", sizeToFit: true),
        // new ListSlide(["Manually Designed", "Randomly Generated"], 75),
        // new ListSlide(["https://github.com/YorkCodeDojo/undergroundmap"], 50),
    ];

    private int _slideNumber = 0;
   
    public MainWindow()
    {
        InitializeComponent();
        this.WindowState = WindowState.Maximized;
        Switcher.Content = _slides[_slideNumber];
        _slides[_slideNumber].Display(true);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.P)
        {
            // P for previous slide
            _slideNumber = Math.Max(0, _slideNumber - 1);
            var previousPage = _slides[_slideNumber];
            Switcher.Content = previousPage;
            previousPage.Display(true);
        }
        
        else if (e.Key == Key.Space)
        {
            // Space bar to either build this slide, or advance to the following slide
            if (_slides[_slideNumber].Display(false) == DisplayResult.Completed)
            {
                // Page is complete, display the next page
                _slideNumber = (_slideNumber + 1) % _slides.Length;
                var nextSlide = _slides[_slideNumber];
                Switcher.Content = nextSlide;
                nextSlide.Display(true);
            }
        }
    }
}