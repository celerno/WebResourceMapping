# WebResourceMapping
This project is part of an interview process. 

## Requirements 
Create an API that contains a single endpoint:

LoadUrl(string url) – when called, the api should download the content of the url. It should return a list of all images, a count of all words, and a count of each word within the content.
Create a website that allows the user to input a url (could be an api or a web page url). When submitted, the site should call the previously created API. Once a response is received, the site should display the following things:
A carousel of images
A total word count
A graph of the top 10 words and their counts

## Setup instructions
The solution contains two main projects called WebResourceMapping (web) and WebResourceMappingAPI (api). Both are build on .net 6. 
In order to run both projects at the same time, the best way to go is by cloning this repo, opening the solution file in visual studio and play "run" with no extra steps.

Otherwise, after cloning this repo, go to each project's folder and perform dotnet run command like this:

```
WebResourceMapping/dotnet run
WebResourceMappingAPI/dotnet run
```

## Screenshots 
![image](https://user-images.githubusercontent.com/2898916/218931826-fd0c5b8f-1add-4b69-be53-c261a8c0b9c1.png)
