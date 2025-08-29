let inputFileToDo = document.getElementById('FileToDo');

function handleClickAddAttachedFile(){
    inputFileToDo.click();
}

async function handleFileSelected(event){
    const files = event.target.files;
    const filesArray = Array.from(files);
    
    const idTodo = todoEditVM.id;
    const formData=new FormData();
    for(var i=0; i<filesArray.length; i++){
        formData.append("files", filesArray[i]);
    }
    const response = await fetch(`${urlAttachments}/${idTodo}`,{
        method: 'POST',
        body: formData
    });
    
    if(!response.ok){
        handleApiError(response);
        return;
    }
    
    const json = await response.json();
    console.log(json);
    inputFileToDo.value = null;
}