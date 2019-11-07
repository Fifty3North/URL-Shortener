var urlInput = document.querySelector("#urlshort");
var codeInput = document.querySelector("#code");
var submitBtn = document.querySelector("#submit"); 
var respArea = document.querySelector("#resp-area");

urlInput.addEventListener('input', function(ev){
    //TODO: add on change event logic
})

submitBtn.onclick = function(ev){
    if(!this.classList.contains("copy")){
        let url = urlInput.value;
        let code = codeInput.value;

        if(!validateURL(url)){
            // TODO: let the user know what was wrong
            RespArea("Does not appear to be a valid URL");
            return null;
        }

        let req = { "url": url, "code": code };

        fetch("/",{
            method:"POST",
            body: JSON.stringify(req), 
            headers:{
                'Content-Type': 'application/json'
            }
        }).then(res => res.json())
        .then(response => {
			if (response.status === "URL already exists") {
				urlInput.value = new URL(window.location) + response.token;
				submitBtn.innerHTML = "Copy";
				submitBtn.classList.add("copy");
            } else if (response.status === "Code already used") {
                RespArea("That code has already been used for the url: " + response.url);
            }
			else if (response.status === "already shortened") {
				urlInput.value = "";
				RespArea("That link has already been shortened by cped.uk");
            }else{
                console.log(response);
                urlInput.value = new URL(window.location)+response;
                submitBtn.innerHTML = "Copy";
                submitBtn.classList.add("copy");
            }
            
        }).catch(error=> console.log(error));
    }else{
        urlInput.select();
        document.execCommand("copy");
    }
}

function validateURL(url){
    var pattern = new RegExp('^(https?:\\/\\/)?'+ 
    '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|'+ 
    '((\\d{1,3}\\.){3}\\d{1,3}))'+ 
    '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*'+
    '(\\?[;&a-z\\d%_.~+=-]*)?'+ 
    '(\\#[-a-z\\d_]*)?$','i'); 
  return !!pattern.test(url);
}

respArea.addEventListener("mouseout", function(ev){
    setTimeout(() => {
        respArea.classList.remove("active");
        respArea.classList.add("inactive");
    }, 1000);
    
})

respArea.onclick = function(ev){
    respArea.classList.remove("active");
    respArea.classList.add("inactive");
}

function RespArea(text){
    respArea.classList.remove("inactive");
    respArea.classList.add("active");
    respArea.innerHTML = text;
}