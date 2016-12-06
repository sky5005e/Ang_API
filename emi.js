var starPrinBala=new Array();
var prinAmot=new Array();
var inteAmot=new Array();
var EMIexcl=new Array();
var serviceTax=new Array();
var EMIserviceTax=new Array();
var endiPrinBala=new Array();
var total_inteAmot=0, total_EMIexcl=0, total_serviceTax=0,total_EMIserviceTax=0;

$(function(){
	onChnageFunc();
	$(".cal").click(function(){
		validation();
	})
	
	$(".reset").click(function(){
		reset();
	})
	
	
	$("#loanAmount, #inteRate").keypress(function (e){
		 if (e.which != 8 && e.which != 0 && e.which != 46 && (e.which < 48 || e.which > 57)) {
			return false;
		 }
   });
   
   $("#tenureMonths, #loanType,#loanAmount, #inteRate").focus(function(){
	   $("html,body").stop(true,true)
   })
})


function validation(){
	
	
	var error='', error2='', error3='', error3='';
	
	
	
	if($('#loanAmount').val()=="0" && $('.tenureMonths .selVal').text()=="Select months"){
		alert("1. Loan amount cannot be zero. Please enter a valid loan amount \n2. Please select the tenure.");
		return false;
	}
	
	if($('#loanAmount').val()=="0" && $('.tenureMonths .selVal').text()!="Select months"){
		alert("Loan amount cannot be zero. \nPlease enter a valid loan amount");
		return false;
	}
	
	
	
	if(isNaN($('#loanAmount').val()) || $('#loanAmount').val()=="") error+='loan amount';

	
	if($('#inteRate').val()==""){
		if(error!='') error+=', ';
		 error+='interest rate';
	}
	if($('.tenureMonths .selVal').text()=="" || $('.tenureMonths .selVal').text()=="Select months" )  error2='select the tenure';
	if(error!='') error3='Please enter the '+error;
	if(error2!='' && error!='') error3+=' and ';
	if(error2!='' && error=='') error3+='Please ';
	if(error2!='') error3+=error2;
	if(error3==''){
		calc();
	}else{
		alert(error3)
		return error;
	}
}

function onChnageFunc(){
	$("select").each(function(){
		$(this)[0].selectedIndex = 0;
	})
	
	$(".selBox select").change(function(){
		$(this).prev().text($(this).find('option:selected').text())
    })
	
	$("#loanAmount").change(function(){
		if(!Number($(this).val()) || $(this).val()=="" || $(this).val()<0) {
			 $(this).val(0);
			 return false;
		}
		if (!/^\d{0,15}(?:\.\d{0,2})?$/.test(this.value)){
			$(this).val($(this).val().replace(/(\d{0,15}.\d{0,2})(.*)/g,"$1"))
		}
	});
	
	$("#inteRate").change(function(){
		if(!Number($(this).val()) || $(this).val()=="" || $(this).val()<0) {
			 $(this).val(0);
			 return false;
		}
		if (!/^\d{0,15}(?:\.\d{0,2})?$/.test(this.value)){
			$(this).val($(this).val().replace(/(\d{0,15}.\d{0,2})(.*)/g,"$1"))
		}
		if($(this).val()>99){
			alert("Please Interest Rate enter less than 99");
			$(this).val(0);
			return false;
		}
	});
	
	$("#loanType").change(function(){
		
		createTenureMon()
		
	})
	
	/*$("#processingFee").change(function(){
		if(isNaN($(this).val()) || $(this).val()=="" || $(this).val()=="")  $(this).val(0);
	});*/
}

function reset(){
	$("select").each(function(){
		$(this)[0].selectedIndex = 0;
		$(this).prev().text($(this).find('option:selected').text())
	})
	
	$("#loanAmount, #inteRate").val("");
	$(".totalDiv, .emiTable-wrap").hide();
	$(".termsCondi").fadeIn()
	$("#m-StarPrinAmot, #m-prinAmot, #inteAmot, #EMIexcl, #serviceTax, #EMIserviceTax").text(0);
}

function calc(){
	$(".reset, .totalDiv, .emiTable-note, .emiTable-wrap").show();
	if($('#loanType option:selected').index()==1) $(".ServicTtaxText").hide()
	if($('#loanType option:selected').index()!=1) $(".ServicTtaxText").show();
	$(".termsCondi").hide();
	
	if($(window).width()<=640){
		$(".mTable").fadeIn();
	}else{
		$(".dTable").fadeIn();
	}
	totlaCal();
}

function totlaCal(){
	var totlaRateAnnum=0,totlaRateMonth=0,emiInteRate=0;
	var loanType=parseInt($("#loanType").val());
	var loanAmount=parseInt($("#loanAmount").val());
	var inteRate=parseFloat	($("#inteRate").val());
	var tenureMonths=parseInt($(".tenureMonths .selVal").text());
	
	//var emiInteRate=0;
	if($("#rateType option:selected").index()==0){
		totlaRateAnnum=inteRate;
		totlaRateMonth=inteRate/12;
		
		emiInteRate=inteRate/12;
		inteRate=inteRate/12;
	}else{
		totlaRateAnnum=inteRate*12;
		totlaRateMonth=inteRate;
		emiInteRate=inteRate;
	}
	
	var processingFee=1
	var emi=(loanAmount*(emiInteRate/100)/(1-Math.pow(1+(emiInteRate/100),(-1*tenureMonths)))*100)/100;
	if(inteRate==0) emi=loanAmount/tenureMonths
		
	var loanTypeMiniAmou=0;
	if(loanType==1 || loanType==2) loanTypeMiniAmou=500.00;
	if(loanType==3 || loanType==4) loanTypeMiniAmou=250.00;

	$("#totlaRateAnnum").text((totlaRateAnnum).toFixed(2)+"%");
	$("#totlaRateMonth").text((totlaRateMonth).toFixed(2)+"%");
	//$("#totalEmi").text(Math.round(emi.toFixed(2)));
	$("#totalEmi").text(Math.ceil(emi));
	////console.log(Math.ceil(emi))
	
	/*if($("#loanType").val()==1){
		$("#totalFee").text(500);
		$("#FeeServTax").text(570)
	}else if($("#loanType").val()==2){
		$("#totalFee").text(1000);
		$("#FeeServTax").text(1140)
	}else{
		$("#totalFee").text(Math.round((loanAmount*processingFee/100).toFixed(2)));
	 	$("#FeeServTax").text(Math.round(( Number($("#totalFee").text())*14/100) + (loanAmount*processingFee/100)));
	}*/
	
	$("#totalFee").text(Math.round((loanAmount*processingFee/100).toFixed(2)));
	$("#FeeServTax").text(Math.round(( Number($("#totalFee").text())*15/100) + (loanAmount*processingFee/100)));
	
	dataRows(loanType,loanAmount,inteRate,tenureMonths,processingFee,emi)
	
	if($(window).width()>640){
		var tableCreate=createRows(tenureMonths,loanType)
		$(".dTable_replace").html(tableCreate)
		
		$("html,body").delay(1200).animate({scrollTop:$(".emiTable-wrap").offset().top},1200)
	}else{
		mobileDataRow();
		$("html,body").delay(1000).animate({scrollTop:$(".mTable").offset().top},1500)
	}
}

function createRows(tenureMonths,loanType){
	var classVal='';
	if(loanType==2) classVal='class="emi-serviceTaxHeader"';
	 var loanAmount=parseInt($("#loanAmount").val());
	
	 var titleTr='<table cellspacing="0" cellpadding="0">';
	 titleTr+='<tr class="headerTr">';
	 titleTr+='<td>Month</td>';
	 titleTr+='<td>Starting Principal Balance</td>';
	 titleTr+='<td>Principal Amount</td>';
	 titleTr+='<td>Interest Amount</td>';
	 titleTr+='<td '+classVal+'>EMI</td>';
	 if(loanType!=2) titleTr+='<td>Service Tax</td>';
	 if(loanType!=2) titleTr+='<td class="emi-serviceTaxHeader">EMI + Service Tax</td>';
	 titleTr+='<td>Ending Principal Balance</td>';
	 titleTr+='</tr>';
	 
	 var dataRow='';
	 for(var i=0;i<tenureMonths; i++){
		 dataRow+='<tr>';
		 dataRow+='<td>'+(i+1)+'</td>';
		 dataRow+='<td>'+Math.round(starPrinBala[i])+'</td>';
		 dataRow+='<td>'+Math.round(prinAmot[i])+'</td>';
		 dataRow+='<td>'+Math.round(inteAmot[i])+'</td>';
		 dataRow+='<td '+classVal+'>'+Math.ceil(EMIexcl[i])+'</td>';
		 if(loanType!=2) dataRow+='<td>'+Math.round(serviceTax[i])+'</td>';
		 if(loanType!=2) dataRow+='<td class="emi-serviceTaxHeader">'+Math.ceil(EMIserviceTax[i])+'</td>';
		 dataRow+='<td>'+Math.round(endiPrinBala[i])+'</td>';
		 dataRow+='</tr>';
	 }
	 
	 var totalTr='<tr class="totalTr">';
	 totalTr+='<td>&nbsp;</td>';
	 totalTr+='<td>Total</td>';
	 totalTr+='<td>'+ Math.round(loanAmount)+'</td>';
	 totalTr+='<td>'+Math.round(total_inteAmot)+'</td>';
	 totalTr+='<td>'+Math.ceil(total_EMIexcl)+'</td>';
	 if(loanType!=2) totalTr+='<td>'+Math.round(total_serviceTax)+'</td>';
	 if(loanType!=2) totalTr+='<td>'+Math.ceil(total_EMIserviceTax)+'</td>';
	 totalTr+='<td>&nbsp;</td>';
	 totalTr+='</tr>';
	 totalTr+='</table>';
	 
	 var tableCreate=titleTr+dataRow+totalTr
	 return tableCreate;
}


function dataRows(loanType,loanAmount,inteRate,tenureMonths,processingFee,emi){
	 var princ=loanAmount, princAm=0, interestAm=0, emiAm=emi,servTax=0, balPrin=0, yearRate=inteRate*12,EMIservTax=0,m_options=''; 
	 starPrinBala.length=0, prinAmot.length=0,inteAmot.length=0, EMIexcl.length=0, serviceTax.length=0, EMIserviceTax.length=0, endiPrinBala.length=0;
	 total_inteAmot=0, total_EMIexcl=0, total_serviceTax=0,total_EMIserviceTax=0;
	
	 for(var i=0;i<tenureMonths; i++){
		interestAm=(yearRate*princ/1200);
		
		princAm=emiAm-interestAm;
		servTax=interestAm*15/100
		balPrin=princ-princAm;
		if(balPrin<0) balPrin=0;
		EMIservTax=emiAm+servTax
		
		starPrinBala.push(princ.toFixed(2));
		prinAmot.push(princAm.toFixed(2));
		inteAmot.push(interestAm.toFixed(2));
		EMIexcl.push(emiAm.toFixed(2));
		serviceTax.push(servTax.toFixed(2));
		EMIserviceTax.push(EMIservTax.toFixed(2));
		endiPrinBala.push(balPrin.toFixed(2));
		
		princ=balPrin;		
		m_options+="<option>"+(i+1)+"</option>"
		
		total_inteAmot+=interestAm, total_EMIexcl+=emiAm, total_serviceTax+=servTax,total_EMIserviceTax+=EMIservTax;
	 }
	 $(".mTable select").html(m_options)
}

function mobileDataRow(){
	$(".mTable .selBox .selVal").text(1);
	$(".mTable select").selectedIndex = 0;
	$("#m-StarPrinAmot").text(Math.round(starPrinBala[0]));
	$("#m-prinAmot").text(Math.round(prinAmot[0]));
	$("#inteAmot").text(Math.round(inteAmot[0]));
	$("#EMIexcl").text(Math.round(EMIexcl[0]));
	$("#serviceTax").text(Math.round(serviceTax[0]));
	$("#EMIserviceTax").text(Math.round(EMIserviceTax[0]));
	$("#endPrinBala").text(Math.round(endiPrinBala[0]));
	
	$(".mTable select").change(function(){
		var selectInd=$(this).find('option:selected').index();
		$("#m-StarPrinAmot").text(Math.round(starPrinBala[selectInd]));
		$("#m-prinAmot").text(Math.round(prinAmot[selectInd]));
		$("#inteAmot").text(Math.round(inteAmot[selectInd]));
		$("#EMIexcl").text(Math.round(EMIexcl[selectInd]));
		$("#serviceTax").text(Math.round(serviceTax[selectInd]));
		$("#EMIserviceTax").text(Math.round(EMIserviceTax[selectInd]));
		$("#endPrinBala").text(Math.round(endiPrinBala[selectInd]));
	})
}

function createTenureMon(){
	var loanType=$("#loanType").val();
	$(".tenureMonths .selVal").text('Select months')
	var month_options="";
	$(".link1,.link2").hide();
	if(loanType==1){
			$(".link1").show();
			
			month_options+='<option>Select months</option>';
			month_options+='<option>12</option>';
            month_options+='<option>18</option>';
			month_options+='<option>24</option>';
			month_options+='<option>36</option>';
			month_options+='<option>48</option>';
			month_options+='<option>60</option>';	
	}
	if(loanType==2){
			$(".link2").show();
			month_options+='<option>Select months</option>';
			month_options+='<option>12</option>';
			month_options+='<option>18</option>';
			month_options+='<option>24</option>';
			month_options+='<option>36</option>';
			month_options+='<option>48</option>';
			month_options+='<option>60</option>';	
	}
	
	
	if(loanType==3 || loanType==4){
			month_options+='<option>Select months</option>';
			month_options+='<option>6</option>';
			month_options+='<option>9</option>';
			month_options+='<option>12</option>';
			month_options+='<option>18</option>';
			month_options+='<option>24</option>';
			month_options+='<option>36</option>';
			month_options+='<option>48</option>';
			
	}
	
	$("#tenureMonths").html(month_options);	
}
