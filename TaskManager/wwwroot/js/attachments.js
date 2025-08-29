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
        
        todoEditVM.attachments.push(new fileAttachedViewModel({...attachment, isEditing: false}));
    });
}

let titleFileAttachedPrevious;
function handleClickTitleFileAttached(fileAttached){
    fileAttached.isEditing(true);
    titleFileAttachedPrevious = fileAttached.title();
    $("[name='txtFileTitle']:visible").focus();
}

async function handleFocusoutTitleFileAttached(fileAttached){
    fileAttached.isEditing(false);
    const idTodo = fileAttached.id;
    
    if(!fileAttached.title())
        fileAttached.title(titleFileAttachedPrevious);
    
    if(fileAttached.title() === titleFileAttachedPrevious)
        return;
    
    const data = JSON.stringify(fileAttached.title())
    const response = await fetch(`${urlAttachments}/${idTodo}`,{
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(!response.ok){
        handleApiError(response);
    }
}