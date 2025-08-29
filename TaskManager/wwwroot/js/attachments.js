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
    prepareAttachments(json)
    inputFileToDo.value = null;
}


function prepareAttachments(attachments){
    
    attachments.forEach(attachment =>{
        let createdAt = attachment.createdAt;
        if(createdAt.indexOf('Z')===-1){
            createdAt += 'Z';
        }
        const createdAtDate = new Date(createdAt);
        attachment.published = createdAtDate.toLocaleDateString();
        
        todoEditVM.attachments.push(attachment);
    });
}