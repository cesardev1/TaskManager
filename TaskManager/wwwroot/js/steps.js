function handleClickAddStep(){
    const index = todoEditVM.steps().findIndex(s=>s.isNew);
    
    if(index !== -1){
        return;
    }
    todoEditVM.steps.push(new stepViewModel({isEditing:true, isCompleted:false}));
    $("[name=txtStepDescription]:visible").focus();
}

function handleClickCancelStep(step){
    if(step.isNew()){
        todoEditVM.steps.pop();
    } else {
        
    }
}

async function handleClickSaveStep(step){
    step.isEditing(false);
    const isNew = step.isNew();
    console.log(todoEditVM);
    const todoId = todoEditVM.id;
    const data = getBodyStep(step);
    
    if(isNew){
        await insertStep(step,data,todoId);
    } else {
        
    }
}

async function insertStep(step,data,todoId){
    const response = await fetch(`${urlSteps}/${todoId}`,{
        body:data,
        method:"POST",
        headers:{
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok){
        const json = await response.json();
        step.id(json.id);
    } else {
        handleApiError(response);
    }
}


function getBodyStep(step){
    return JSON.stringify({
        description: step.description(),
        isCompleted: step.isCompleted()
    });
}
